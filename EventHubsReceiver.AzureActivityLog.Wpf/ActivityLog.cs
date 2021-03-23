using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventHubsReceiver.AzureActivityLog.Wpf
{
    public class Evidence
    {
        public string role { get; set; }
    }

    public class Authorization
    {
        public string scope { get; set; }
        public string action { get; set; }
        public Evidence evidence { get; set; }
    }

    public class Claims
    {
        public string aud { get; set; }
        public string iss { get; set; }
        public string iat { get; set; }
        public string nbf { get; set; }
        public string exp { get; set; }

        [JsonProperty("http://schemas.microsoft.com/claims/authnclassreference")]
        public string HttpSchemasMicrosoftComClaimsAuthnclassreference { get; set; }
        public string aio { get; set; }
        public string altsecid { get; set; }

        [JsonProperty("http://schemas.microsoft.com/claims/authnmethodsreferences")]
        public string HttpSchemasMicrosoftComClaimsAuthnmethodsreferences { get; set; }
        public string appid { get; set; }
        public string appidacr { get; set; }

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")]
        public string HttpSchemasXmlsoapOrgWs200505IdentityClaimsEmailaddress { get; set; }

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")]
        public string HttpSchemasXmlsoapOrgWs200505IdentityClaimsSurname { get; set; }

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")]
        public string HttpSchemasXmlsoapOrgWs200505IdentityClaimsGivenname { get; set; }
        public string groups { get; set; }

        [JsonProperty("http://schemas.microsoft.com/identity/claims/identityprovider")]
        public string HttpSchemasMicrosoftComIdentityClaimsIdentityprovider { get; set; }
        public string ipaddr { get; set; }
        public string name { get; set; }

        [JsonProperty("http://schemas.microsoft.com/identity/claims/objectidentifier")]
        public string HttpSchemasMicrosoftComIdentityClaimsObjectidentifier { get; set; }
        public string puid { get; set; }
        public string rh { get; set; }

        [JsonProperty("http://schemas.microsoft.com/identity/claims/scope")]
        public string HttpSchemasMicrosoftComIdentityClaimsScope { get; set; }

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")]
        public string HttpSchemasXmlsoapOrgWs200505IdentityClaimsNameidentifier { get; set; }

        [JsonProperty("http://schemas.microsoft.com/identity/claims/tenantid")]
        public string HttpSchemasMicrosoftComIdentityClaimsTenantid { get; set; }

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")]
        public string HttpSchemasXmlsoapOrgWs200505IdentityClaimsName { get; set; }
        public string uti { get; set; }
        public string ver { get; set; }
        public string wids { get; set; }
        public string xms_tcdt { get; set; }
    }

    public class Identity
    {
        public Authorization authorization { get; set; }
        public Claims claims { get; set; }
    }

    public class Properties
    {
        public string requestbody { get; set; }
        public string eventCategory { get; set; }
        public string entity { get; set; }
        public string message { get; set; }
        public string hierarchy { get; set; }
        public string statusCode { get; set; }
        public object serviceRequestId { get; set; }
        public string responseBody { get; set; }
    }

    public class Record
    {
        public string RoleLocation { get; set; }
        public DateTime time { get; set; }
        public string resourceId { get; set; }
        public string operationName { get; set; }
        public string category { get; set; }
        public string resultType { get; set; }
        public string resultSignature { get; set; }
        public string durationMs { get; set; }
        public string callerIpAddress { get; set; }
        public string correlationId { get; set; }
        public Identity identity { get; set; }
        public string level { get; set; }
        public Properties properties { get; set; }
        public string tenantId { get; set; }
    }

    public class ActivityLog
    {
        public List<Record> records { get; set; }
    }


}
