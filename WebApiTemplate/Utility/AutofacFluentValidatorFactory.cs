using System;
using Autofac;
using FluentValidation;

namespace webapitmpl.Utility
{
    /// <summary>
    /// FluentValidation Validator factory bridging validators registered in a container
    /// </summary>
    public class AutofacFluentValidatorFactory : IValidatorFactory
    {
        private IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacFluentValidatorFactory"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacFluentValidatorFactory(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Gets the validator for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IValidator GetValidator(Type type)
        {
            var targetType = typeof(IValidator<>).MakeGenericType(type);
            return (IValidator)container.ResolveOptional(targetType);
        }

        /// <summary>
        /// Gets the validator for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IValidator<T> GetValidator<T>()
        {
            return container.ResolveOptional<IValidator<T>>();
        }
    }
}
