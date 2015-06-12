using FluentValidation;
using FluentValidation.Attributes;

namespace webapitmpl.Models
{
    /// <summary>
    /// Validation rules common for all operations
    /// </summary>
    public class WidgetValidator : AbstractValidator<Widget>
    {
        public WidgetValidator()
        {
            RuleFor(x => x.Name).Length(1, 20);
            RuleFor(x => x.Description).Length(0, 100);
        }
    }

    public class WidgetCreate : Widget { }


    /// <summary>
    /// Validation rules for create
    /// </summary>
    public class WidgetCreateValidator : WidgetValidator
    {
        public WidgetCreateValidator()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }

    public class WidgetUpdate : Widget { }

    /// <summary>
    /// Validation rules for update
    /// </summary>
    public class WidgetUpdateValidator : WidgetValidator
    {
        public WidgetUpdateValidator()
        {
        }
    }
}
