using System;

namespace webapitmpl.Models
{
    /// <summary>
    /// A widget
    /// </summary>
    public class Widget
    {
        /// <summary>
        /// The Widget's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A human readable description of the widget
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date and time when the widget was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
