using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClamAvProxy
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FileContentAttribute : Attribute { }
    
    public class FileContentFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attribute = context.MethodInfo.GetCustomAttributes(typeof(FileContentAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                return;
            }

            operation.RequestBody = new OpenApiRequestBody { Required = true };
            operation.RequestBody.Content.Add("application/octet-stream", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "string"
                },
            });
        }
    }
}