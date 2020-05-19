using System;
using System.Collections.Generic;
using Calamari.Aws.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Octopus.CoreUtilities.Extensions;

namespace Calamari.Aws.Integration.S3
{
    public class VariableS3TargetOptionsProvider : IProvideS3TargetOptions
    {
        readonly IVariables variables;


        public VariableS3TargetOptionsProvider(IVariables variables)
        {
            this.variables = variables;
        }

        IEnumerable<S3FileSelectionProperties> GetFileSelections()
        {
            return variables.Get(SpecialVariableNames.Aws.S3.FileSelections)
                ?.Map(Deserialize<List<S3FileSelectionProperties>>);
        }

        S3PackageOptions GetPackageOptions()
        {
            return variables.Get(SpecialVariableNames.Aws.S3.PackageOptions)
                ?.Map(Deserialize<S3PackageOptions>);
        }

        static JsonSerializerSettings GetEnrichedSerializerSettings()
        {
            return JsonSerialization.GetDefaultSerializerSettings()
                .Tee(x =>
                {
                    x.Converters.Add(new FileSelectionsConverter());
                    x.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, GetEnrichedSerializerSettings());
        }

        public IEnumerable<S3TargetPropertiesBase> GetOptions(S3TargetMode mode)
        {
            switch (mode)
            {
                case S3TargetMode.EntirePackage:
                    return new List<S3TargetPropertiesBase>{GetPackageOptions()};
                case S3TargetMode.FileSelections:
                    return GetFileSelections();
                default:
                    throw new ArgumentOutOfRangeException("Invalid s3 target mode provided", nameof(mode));
            }
        }
    }
}