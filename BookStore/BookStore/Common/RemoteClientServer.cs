using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace BookStore.Common
{
    public class RemoteClientServerAttribute : RemoteAttribute
    {
        #region "IsValid"
        /// <summary>Validates the specified value with respect to the current validation attribute.</summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the controller using reflection
            Type controller = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(type => type.Name.ToLower() == string.Format("{0}Controller",
                    this.RouteData["controller"].ToString()).ToLower());
            if (controller != null)
            {
                // Get the action method that has validation logic
                MethodInfo action = controller.GetMethods()
                    .FirstOrDefault(method => method.Name.ToLower() ==
                        this.RouteData["action"].ToString().ToLower());
                if (action != null)
                {
                    // Create an instance of the controller class
                    object instance = Activator.CreateInstance(controller);
                    // Invoke the action method that has validation logic

                    object response = action.Invoke(instance, new object[] { value });
                    if (response is JsonResult)
                    {
                        object jsonData = ((JsonResult)response).Data;
                        if (jsonData is bool)
                        {
                            return (bool)jsonData ? ValidationResult.Success :
                                new ValidationResult(this.ErrorMessage);
                        }
                    }
                }
            }

            return ValidationResult.Success;
            // If you want the validation to fail, create an instance of ValidationResult
            // return new ValidationResult(base.ErrorMessageString);
        }
        #endregion

        #region "RemoteClientServerAttribute"
        /// <summary>Initializes a new instance of the <see cref="T:BookStore.Common.RemoteClientServerAttribute"/> class.</summary>
        /// <param name="routeName">The route name.</param>
        public RemoteClientServerAttribute(string routeName)
            : base(routeName)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:BookStore.Common.RemoteClientServerAttribute"/> class.</summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        public RemoteClientServerAttribute(string action, string controller)
            : base(action, controller)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:BookStore.Common.RemoteClientServerAttribute"/> class.</summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="areaName">The name of the area.</param>
        public RemoteClientServerAttribute(string action, string controller,
                string areaName) : base(action, controller, areaName)
        {
        }
        #endregion
    }
}