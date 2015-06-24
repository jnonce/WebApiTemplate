using System.Collections.Generic;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Swagger operation filter which adds parameters to all filtered APIs
    /// </summary>
    internal class AddedParametersSwaggerOperationFilter : IOperationFilter
    {
        private Parameter[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddedParametersSwaggerOperationFilter"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public AddedParametersSwaggerOperationFilter(params Parameter[] parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiDescription">The API description.</param>
        public void Apply(
            Operation operation,
            SchemaRegistry schemaRegistry,
            ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>(parameters);
            }
            else
            {
                foreach (Parameter p in parameters)
                {
                    operation.parameters.Add(p);
                }
            }
        }
    }
}
