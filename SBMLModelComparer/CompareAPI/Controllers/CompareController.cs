using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Compare;
using Compare.DataModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompareAPI.Controllers
{
    [Route("api/[controller]")]
    public class CompareController : Controller
    {
        private IHostingEnvironment Env { get; }

        public class ModelsPost
        {
            public IFormFile FileModelA { get; set; }

            public IFormFile FileModelB { get; set; }
            //public IFormFile FileBaseModel { get; set; }
        }

        private class UploadedFile
        {
            public string Path { get; }

            public ModelFileName ModelName { get; }

            public string TempPath { get; }

            public UploadedFile(string path, string tempPath, ModelFileName modelName)
            {
                Path = path;
                TempPath = tempPath;
                ModelName = modelName;
            }
        }

        private enum ModelFileName
        {
            ModelA,
            ModelB,
            BaseModel
        }

        public CompareController(IHostingEnvironment env)
        {
            Env = env;
        }

        // GET: api/Compare
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"ModelDiffer", "Online"};
        }

        [HttpPost]
        [EnableCors("MyCrossPolicy")]
        public async Task<IActionResult> Post([FromForm] ModelsPost modelsPost)
        {
            var tempUploadedModelList = new List<UploadedFile>();
            try
            {
                Console.WriteLine("You received the call!");

                var modelAFilePath = await CreateTempUploadedFile(modelsPost.FileModelA);
                if (modelAFilePath != string.Empty)
                    tempUploadedModelList.Add(new UploadedFile(modelsPost.FileModelA.FileName, modelAFilePath, ModelFileName.ModelA));
                else
                    return BadRequest("Problems loading the Model A file!");

                var modelBFilePath = await CreateTempUploadedFile(modelsPost.FileModelB);
                if (modelBFilePath != string.Empty)
                    tempUploadedModelList.Add(new UploadedFile(modelsPost.FileModelB.FileName, modelBFilePath, ModelFileName.ModelB));
                else
                    return BadRequest("Problems loading the Model B file!");

                var mappedPath = Path.Combine(Env.WebRootPath, "SBMLModels/HMR_2_0.xml");
                tempUploadedModelList.Add(new UploadedFile(mappedPath, mappedPath, ModelFileName.BaseModel));
                
                var jsonOut = GetModelComparionResult(tempUploadedModelList);

                CleanTempUploadedFiles(tempUploadedModelList);

                return Ok(jsonOut);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception generated when uploading files - " + exp.Message);
                CleanTempUploadedFiles(tempUploadedModelList);
                string message = $"file / upload failed!";
                return Json(message);
            }
        }

        private static ModelComparisonResult GetModelComparionResult(List<UploadedFile> tempUploadedModelList)
        {
            ModelComparisonResult jsonOut;
            var modelA = tempUploadedModelList.First(f => f.ModelName == ModelFileName.ModelA);
            var modelB = tempUploadedModelList.First(f => f.ModelName == ModelFileName.ModelB);
            var modelBase = tempUploadedModelList.First(f => f.ModelName == ModelFileName.BaseModel);

            using (var fileStreamA = System.IO.File.Open(modelA.TempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var fileStreamB = System.IO.File.Open(modelB.TempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var fileStreamBase = System.IO.File.Open(modelBase.TempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        jsonOut = ModelComparer.GetModelDiferences(fileStreamA, modelA.Path, fileStreamB, modelB.Path, fileStreamBase, modelBase.Path);
                    }
                }
            }
            return jsonOut;
        }

        private static void CleanTempUploadedFiles(List<UploadedFile> tempUploadedModelList)
        {
            foreach (var uploadedFile in tempUploadedModelList)
            {
                if (uploadedFile.ModelName != ModelFileName.BaseModel)
                    System.IO.File.Delete(uploadedFile.TempPath);
            }
        }

        private static async Task<string> CreateTempUploadedFile(IFormFile formFile)
        {
            var fileName = string.Empty;
            if (!(formFile?.Length > 0)) return fileName;

            fileName = Path.GetTempFileName();
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
                return fileName;
            }
        }
    }
}
