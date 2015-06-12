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
        
        public AutofacFluentValidatorFactory(IContainer container)
        {
            this.container = container;
        }

        public IValidator GetValidator(Type type)
        {
            var targetType = typeof(IValidator<>).MakeGenericType(type);
            return (IValidator)container.ResolveOptional(targetType);
        }

        public IValidator<T> GetValidator<T>()
        {
            return container.ResolveOptional<IValidator<T>>();
        }
    }
}
