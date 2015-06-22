using FluentValidation;
using FluentValidation.Attributes;

namespace webapitmpl.Models
{
    /// <summary>
    /// Validation rules common for all operations
    /// </summary>
    public class WidgetValidator : AbstractValidator<Widget>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetValidator"/> class.
        /// </summary>
        public WidgetValidator()
        {
            RuleFor(x => x.Name).Length(1, 20);
            RuleFor(x => x.Description).Length(0, 100);
        }
    }

    /// <summary>
    /// Widget used to create
    /// </summary>
    [Validator(typeof(WidgetCreateValidator))]
    public class WidgetCreate : Widget { }


    /// <summary>
    /// Validation rules for create
    /// </summary>
    public class WidgetCreateValidator : WidgetValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetCreateValidator"/> class.
        /// </summary>
        public WidgetCreateValidator()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }

    /// <summary>
    /// Widget used during update
    /// </summary>
    [Validator(typeof(WidgetUpdateValidator))]
    public class WidgetUpdate : Widget { }

    /// <summary>
    /// Validation rules for update
    /// </summary>
    public class WidgetUpdateValidator : WidgetValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetUpdateValidator"/> class.
        /// </summary>
        public WidgetUpdateValidator()
        {
        }
    }
}
