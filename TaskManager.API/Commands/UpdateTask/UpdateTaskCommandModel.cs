using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManager.API.Constants;
using TaskManager.API.DTOs;
using TaskManager.API.Helpers;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Commands
{
    [Serializable]
    public class UpdateTaskCommandModel : IRequest<TaskResponse>, IValidatableObject
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string DueDate { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public string Status { get; set; }

        public UpdateTaskCommandModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="priority"></param>
        /// <param name="status"></param>
        public UpdateTaskCommandModel(string id, string name, string description, string dueDate, string startDate, string endDate
            , string priority, string status)
        {
            Id = id;
            Name = name;
            Description = description;
            DueDate = dueDate;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
            Status = status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate Entered Dates are in valid format or not
            if (!Helper.IsValidDateFormat(DueDate) || !Helper.IsValidDateFormat(StartDate) || !Helper.IsValidDateFormat(EndDate))
            {
                yield return new ValidationResult(Messages.DateWithInvalidFormatMassage);
            }
            else
            {
                //Valiate Task Due Date
                if (DateTime.Compare(Convert.ToDateTime(DueDate), DateTime.Today) < 0)
                {
                    yield return new ValidationResult(Messages.DueDateValidateMassage);
                }

                //Validate Task StartDate
                if (DateTime.Compare(Convert.ToDateTime(StartDate), Convert.ToDateTime(DueDate)) > 0)
                {
                    yield return new ValidationResult(Messages.StartDateValidateMessage);
                }

                //Validate Task EndDate
                if (DateTime.Compare(Convert.ToDateTime(EndDate), Convert.ToDateTime(StartDate)) < 0)
                {
                    yield return new ValidationResult(Messages.EndDateValidateMessage);
                }

                //Validate Priority Value
                if (!Enum.IsDefined(typeof(Priority), Priority))
                {
                    yield return new ValidationResult(Messages.PriorityValidationMessage);
                }

                //Validate Priority Value
                if (!Enum.IsDefined(typeof(Status), Status))
                {
                    yield return new ValidationResult(Messages.StatusValidationMessage);
                }
            }
        }
    }
}
