using SciaTools.ThirdParty.SDK.Services.Factory;
using SciaTools.ThirdParty.SDK.Services.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using SciaTools.ThirdParty.SDK.Models.Commands;
using SciaTools.ThirdParty.SDK.Models.Configuration;
using SciaTools.ThirdParty.SDK.Services.Infrastructure;

namespace SafConnection
{
    public class ThirdPartyCommunicator
    {
        private readonly IThirdPartySdkIntegrator integrator;
        private readonly IApiLogger loggerApi;
        private ApiConfiguration apiConfiguration;

        public ThirdPartyCommunicator()
        {
            loggerApi = new ApiLoggerAction(SetApiLogText);
            integrator = ApiFactory.CreateSdkIntegrator(loggerApi);
        }

        public void Connect(string userConfigPath)
        {
            if (!File.Exists(userConfigPath))
            {
                SetApiLogText("Api not started !");
                return;
            }
            apiConfiguration = integrator.Init(userConfigPath, OnNewApiResponse, (switchContext) => { });
            if (apiConfiguration == null)
            {
                SetApiLogText("Api not started !");
            }
            else
            {
                SetApiLogText("Api CONNECTED !");
            }
        }

        public FileInfo ReadAllSafCommand_Execute()
        {
            SetApiLogText($"{nameof(ReadAllSafCommand_Execute)}");
            if (apiConfiguration == null)
            {
                return null;
            }
            var fileInfo = new FileInfo(Path.Combine(GetTempPath(), "SafRead.xlsx"));
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            integrator.ExecuteReadAllSaf(
                new ApiFileInfo(fileInfo.DirectoryName, fileInfo.Name),
                ReadApiResultSetting());
            return fileInfo;
        }

        public void WriteAllSafCommand_Execute(string fullPath)
        {
            SetApiLogText($"{nameof(WriteAllSafCommand_Execute)}");
            if (apiConfiguration == null)
            {
                return;
            }
            var fileInfo = new FileInfo(fullPath);
            integrator.ExecuteWriteSafAll(new ApiFileInfo(fileInfo.DirectoryName ?? GetTempPath(), fileInfo.Name));
        }

        /// <summary>
        /// Provide changes in application
        /// </summary>
        /// <param name="response"></param>
        private void OnNewApiResponse(ApiResponse response)
        {
            SetApiLogText($"Response: {response.Source.Identification.Type}, id: {response.Source.Identification.Id}");
            SetApiLogText($"{response.Status}: {response.Message}");
        }

        private void SetApiLogText(string msg)
        {
            Console.WriteLine(msg);
        }

        private string GetTempPath()
        {
            return apiConfiguration?.TempFolder ?? Environment.CurrentDirectory;
        }

        private ApiResultSetting ReadApiResultSetting()
        {
            return new ApiResultSetting(
                false, false, false, new List<string>(), new List<string>());
        }
    }
}
