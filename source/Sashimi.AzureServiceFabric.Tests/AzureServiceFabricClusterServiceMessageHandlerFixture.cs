using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Data.Model;
using Octostache;
using Sashimi.AzureServiceFabric.Endpoints;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;
using AzureServiceFabricServiceMessageNames = Sashimi.AzureServiceFabric.AzureServiceFabricClusterServiceMessageHandler.AzureServiceFabricServiceMessageNames;

namespace Sashimi.AzureServiceFabric.Tests
{
    public class AzureServiceFabricClusterServiceMessageHandlerFixture
    {
        ICreateTargetServiceMessageHandler serviceMessageHandler;

        [SetUp]
        public void SetUp()
        {
            serviceMessageHandler = new AzureServiceFabricClusterServiceMessageHandler();
        }

        [Test]
        public void Ctor_Properties_ShouldBeInitializedProperly()
        {
            serviceMessageHandler.AuditEntryDescription.Should().Be("Azure Service Fabric Target");
            serviceMessageHandler.ServiceMessageName.Should().Be(AzureServiceFabricServiceMessageNames.CreateTargetName);
        }

        //[Test]
        //public void IsServiceMessageValid_WhenMessagePropertiesIsNull_ShouldThrowArgumentNullException()
        //{
        //    Action action = () => serviceMessageHandler.IsServiceMessageValid(null, new VariableDictionary());

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Test]
        //public void IsServiceMessageValid_WhenVariableDictionaryIsNull_ShouldThrowArgumentNullException()
        //{
        //    Action action = () => serviceMessageHandler.IsServiceMessageValid(new Dictionary<string, string>(), null);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Test]
        //[TestCaseSource(nameof(AllPossibleMissingPropertyCombinationsForSecureClientCertificate))]
        //public void IsServiceMessageValid_WhenSecurityModeIsSecureClientCertificateAndAnyPropertyIsMissing_ShouldReturnInvalidResult(
        //    (string, IEnumerable<IDictionary<string, string>>) testCase)
        //{
        //    var (securityModeValue, missingPropertyCombinations) = testCase;

        //    foreach (var missingPropertyCombination in missingPropertyCombinations)
        //    {
        //        missingPropertyCombination.Add(AzureServiceFabricServiceMessageNames.SecurityModeAttribute, securityModeValue);
                
        //        var result = serviceMessageHandler.IsServiceMessageValid(missingPropertyCombination, new VariableDictionary());

        //        AssertInvalidValidationResult(result, missingPropertyCombination);
        //    }
        //}

        //[Test]
        //[TestCaseSource(nameof(AllAliasesForSecureClientCertificate))]
        //public void IsServiceMessageValid_WhenSecurityModeIsSecureClientCertificateAndNoPropertyIsMissing_ShouldReturnValidResult(
        //    string securityModeValue)
        //{
        //    var result = serviceMessageHandler.IsServiceMessageValid(GetMessagePropertiesBySecurityMode(securityModeValue), new VariableDictionary());

        //    result.IsValid.Should().BeTrue();
        //    result.Messages.Should().BeEmpty();
        //}

        //[Test]
        //[TestCaseSource(nameof(AllPossibleMissingPropertyCombinationsForAzureActiveDirectory))]
        //public void IsServiceMessageValid_WhenSecurityModeIsActiveDirectoryAndAnyPropertyIsMissing_ShouldReturnInvalidResult(
        //    (string, IEnumerable<IDictionary<string, string>>) testCase)
        //{
        //    var (securityModeValue, missingPropertyCombinations) = testCase;

        //    foreach (var missingPropertyCombination in missingPropertyCombinations)
        //    {
        //        missingPropertyCombination.Add(AzureServiceFabricServiceMessageNames.SecurityModeAttribute, securityModeValue);

        //        var result = serviceMessageHandler.IsServiceMessageValid(missingPropertyCombination, new VariableDictionary());
                
        //        AssertInvalidValidationResult(result, missingPropertyCombination);
        //    }
        //}

        //[Test]
        //[TestCaseSource(nameof(AllAliasesForAzureActiveDirectory))]
        //public void IsServiceMessageValid_WhenSecurityModeIsActiveDirectoryAndNoPropertyIsMissing_ShouldReturnValidResult(
        //    string securityModeValue)
        //{
        //    var result = serviceMessageHandler.IsServiceMessageValid(GetMessagePropertiesBySecurityMode(securityModeValue), new VariableDictionary());

        //    result.IsValid.Should().BeTrue();
        //    result.Messages.Should().BeEmpty();
        //}

        //[Test]
        //public void IsServiceMessageValid_WhenSecurityModeIsUnSecureAndAnyPropertyIsMissing_ShouldReturnInvalidResult()
        //{
        //    var messageProperties = new Dictionary<string, string>
        //    {
        //        {AzureServiceFabricServiceMessageNames.SecurityModeAttribute, "whatever"},
        //        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
        //    };

        //    var result = serviceMessageHandler.IsServiceMessageValid(messageProperties, new VariableDictionary());

        //    AssertInvalidValidationResult(result, messageProperties);
        //}

        //[Test]
        //public void IsServiceMessageValid_WhenSecurityModeIsUnSecureAndNoPropertyIsMissing_ShouldReturnValidResult()
        //{
        //    var result = serviceMessageHandler.IsServiceMessageValid(GetMessagePropertiesBySecurityMode("NotSecured"), new VariableDictionary());

        //    result.IsValid.Should().BeTrue();
        //    result.Messages.Should().BeEmpty();
        //}

        [Test]
        public void BuildEndpoint_WhenMessagePropertiesIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(null, new VariableDictionary(), _ => null, _ => null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenVariableDictionaryIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(new Dictionary<string, string>(), null, _ => null, _ => null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenSecurityModeIsSecureClientCertificateAndCertificateIdResolverIsNull_ShouldThrowArgumentNullException()
        {
            var messageProperties = GetMessagePropertiesBySecurityMode(AllAliasesForSecureClientCertificate().First());
            Action action = () => serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), _ => null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void BuildEndpoint_WhenSecureModeIsSecureClientCertificateButUnableToResolveCertificateId_ShouldThrowException(
            string invalidCertificateId)
        {
            var messageProperties = GetMessagePropertiesBySecurityMode(AllAliasesForSecureClientCertificate().First());

            Action action = () => serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), null,
                _ => invalidCertificateId, null);

            action.Should().Throw<Exception>().Which.Message.Should().Be(
                $"Certificate with Id / Name {messageProperties[AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute]} not found.");
        }

        [Test]
        public void BuildEndpoint_WhenSecureModeIsSecureClientCertificateAndCertificateStoreNameIsMissing_ShouldReturnEndpointWithCorrectProperties()
        {
            var messageProperties = GetMessagePropertiesBySecurityMode(AllAliasesForSecureClientCertificate().First());
            messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreNameAttribute] = null;

            const string certificateId = "Certificates-1";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), null, _ => certificateId, null);

            AssertEndpoint(endpoint, new ExpectedEndpointValues
            {
                SecurityMode = AzureServiceFabricSecurityMode.SecureClientCertificate,
                ConnectionEndpoint = messageProperties[AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute],
                ClientCertVariable = certificateId,
                ServerCertThumbprint = messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute],
                CertificateStoreLocation = messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreLocationAttribute],
                CertificateStoreName = messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreNameAttribute]
            });
        }

        [Test]
        [TestCaseSource(nameof(AllAliasesForSecureClientCertificate))]
        public void BuildEndpoint_WhenSecureModeIsSecureClientCertificateAndCertificateStoreNameIsNotMissing_ShouldReturnEndpointWithCorrectProperties(
            string securityModeValue)
        {
            var messageProperties = GetMessagePropertiesBySecurityMode(securityModeValue);
            const string certificateId = "Certificates-1";
            
            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), null, _ => certificateId, null);

            AssertEndpoint(endpoint, new ExpectedEndpointValues
            {
                SecurityMode = AzureServiceFabricSecurityMode.SecureClientCertificate,
                ConnectionEndpoint = messageProperties[AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute],
                ClientCertVariable = certificateId,
                ServerCertThumbprint = messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute],
                CertificateStoreLocation = messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreLocationAttribute],
                CertificateStoreName = "My"
            });
        }

        [Test]
        [TestCaseSource(nameof(AllAliasesForAzureActiveDirectory))]
        public void BuildEndpoint_WhenSecureModeIsAzureActiveDirectory_ShouldReturnEndpointWithCorrectProperties(
            string securityModeValue)
        {
            var messageProperties = GetMessagePropertiesBySecurityMode(securityModeValue);
            const string certificateId = "Certificates-3";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), null, _ => certificateId, null);

            AssertEndpoint(endpoint, new ExpectedEndpointValues
            {
                SecurityMode = AzureServiceFabricSecurityMode.SecureAzureAD,
                ConnectionEndpoint = messageProperties[AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute],
                ServerCertThumbprint = messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute],
                AadCredentialType = AzureServiceFabricCredentialType.UserCredential,
                AadUserCredentialUsername = messageProperties[AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute],
                AadUserCredentialPassword = new SensitiveString(messageProperties[AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute])
            });
        }

        [Test]
        public void BuildEndpoint_WhenSecureModeIsUnSecure_ShouldReturnEndpointWithCorrectProperties()
        {
            var messageProperties = GetMessagePropertiesBySecurityMode("NotSecuredAtAll");
            
            const string certificateId = "Certificates-5";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), null, _ => certificateId, null);

            AssertEndpoint(endpoint, new ExpectedEndpointValues
            {
                SecurityMode = AzureServiceFabricSecurityMode.Unsecure,
                ConnectionEndpoint = messageProperties[AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute],
            });
        }

        static void AssertInvalidValidationResult(ServiceMessageValidationResult validationResult, IDictionary<string, string> messageProperties)
        {
            validationResult.IsValid.Should().BeFalse();
            validationResult.Messages.Length.Should().Be(messageProperties.Values.Count(v => v == null));
            foreach (var keyValuePair in messageProperties.Where(p => p.Value == null))
            {
                switch (keyValuePair.Key)
                {
                    case AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute:
                        validationResult.Messages.Should().ContainSingle(m => m == "Certificate Thumbprint is missing");
                        break;
                    case AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute:
                        validationResult.Messages.Should().ContainSingle(m => m == "Active Directory Username is missing");
                        break;
                    case AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute:
                        validationResult.Messages.Should().ContainSingle(m => m == "Active Directory Password is missing");
                        break;
                    case AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute:
                        validationResult.Messages.Should().ContainSingle(m => m == "Connection endpoint is missing");
                        break;
                }
            }
        }

        static void AssertEndpoint(Endpoint actualEndpoint, ExpectedEndpointValues expectedEndpointValues)
        {
            actualEndpoint.Should().BeOfType<AzureServiceFabricClusterEndpoint>();
            var serviceFabricClusterEndpoint = (AzureServiceFabricClusterEndpoint) actualEndpoint;
            serviceFabricClusterEndpoint.SecurityMode.Should().Be(expectedEndpointValues.SecurityMode);
            serviceFabricClusterEndpoint.ConnectionEndpoint.Should().Be(expectedEndpointValues.ConnectionEndpoint);

            switch (serviceFabricClusterEndpoint.SecurityMode)
            {
                case AzureServiceFabricSecurityMode.SecureClientCertificate:
                    serviceFabricClusterEndpoint.ClientCertVariable.Should().Be(expectedEndpointValues.ClientCertVariable);
                    serviceFabricClusterEndpoint.ServerCertThumbprint.Should().Be(expectedEndpointValues.ServerCertThumbprint);
                    serviceFabricClusterEndpoint.CertificateStoreLocation.Should().Be(expectedEndpointValues.CertificateStoreLocation);
                    serviceFabricClusterEndpoint.CertificateStoreName.Should().Be(expectedEndpointValues.CertificateStoreName);
                    break;
                case AzureServiceFabricSecurityMode.SecureAzureAD:
                    serviceFabricClusterEndpoint.ServerCertThumbprint.Should().Be(expectedEndpointValues.ServerCertThumbprint);
                    serviceFabricClusterEndpoint.AadUserCredentialUsername.Should().Be(expectedEndpointValues.AadUserCredentialUsername);
                    serviceFabricClusterEndpoint.AadUserCredentialPassword.Should().Be(expectedEndpointValues.AadUserCredentialPassword);
                    serviceFabricClusterEndpoint.AadCredentialType.Should().Be(expectedEndpointValues.AadCredentialType);
                    break;
            }
        }

        static IDictionary<string, string> GetMessagePropertiesBySecurityMode(string securityModeValue)
        {
            var messageProperties = new Dictionary<string, string>
            {
                [AzureServiceFabricServiceMessageNames.SecurityModeAttribute] = securityModeValue,
                [AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute] = "Connection"
            };

            if (AllAliasesForSecureClientCertificate()
                .Any(a => a.Equals(securityModeValue, StringComparison.OrdinalIgnoreCase)))
            {
                messageProperties[AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute] = "Certificate";
                messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute] = "Thumbprint";
                messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreLocationAttribute] = "Location";
                messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreNameAttribute] = "StoreName";

                return messageProperties;
            }

            if (AllAliasesForAzureActiveDirectory()
                .Any(a => a.Equals(securityModeValue, StringComparison.OrdinalIgnoreCase)))
            {
                messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute] = "Thumbprint";
                messageProperties[AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute] = "Username";
                messageProperties[AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute] = "Password";

                return messageProperties;
            }

            return messageProperties;
        }

        static IEnumerable<(string securityModeValue, IEnumerable<IDictionary<string, string>> missingPropertyCombinations)> AllPossibleMissingPropertyCombinationsForSecureClientCertificate()
        {
            foreach (var aliase in AllAliasesForSecureClientCertificate())
            {
                yield return (aliase, new List<IDictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, "Certificate"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, "Certificate"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, "Certificate"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    }
                });
            }
        }

        static IEnumerable<string> AllAliasesForSecureClientCertificate()
        {
            return new[] {"secureclientcertificate", "clientcertificate", "clientCertificate", "certificate", "certiFicate" };
        }

        static IEnumerable<(string securityModeValue, IEnumerable<IDictionary<string, string>>missingPropertyCombinations)> AllPossibleMissingPropertyCombinationsForAzureActiveDirectory()
        {
            foreach (var aliase in AllAliasesForAzureActiveDirectory())
            {
                yield return (aliase, new List<IDictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, "Thumbprint"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, "Username"},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, null}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, "Password"},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    },
                    new Dictionary<string, string>
                    {
                        {AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute, null},
                        {AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute, "Connection"}
                    }
                });
            }
        }

        static IEnumerable<string> AllAliasesForAzureActiveDirectory()
        {
            return new[] { "aad", "aAd", "azureactivedirectory", "azureActiveDirectory" };
        }

        class ExpectedEndpointValues
        {
            public AzureServiceFabricSecurityMode SecurityMode { get; set; }
            public string ConnectionEndpoint { get; set; }
            public string ClientCertVariable { get; set; }
            public string ServerCertThumbprint { get; set; }
            public string CertificateStoreLocation { get; set; }
            public string CertificateStoreName { get; set; }
            public string AadUserCredentialUsername { get; set; }
            public SensitiveString AadUserCredentialPassword { get; set; }
            public AzureServiceFabricCredentialType AadCredentialType { get; set; }
        }
    }
}