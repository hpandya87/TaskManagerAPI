using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManager.API.Constants;
using TaskManager.API.DTOs;

namespace TaskManager.API.Commands
{
    public class DeleteTaskCommandModel : IRequest<TaskResponse>, IValidatableObject
    {
        [Required]
        public string Id { get; set; }

        public DeleteTaskCommandModel() { }

        public DeleteTaskCommandModel(string id)
        {
            Id = id;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Valiate Id Length
            if (Id.Length > 50)
            {
                yield return new ValidationResult(Messages.IdMaximumAllowedLengthMessage);
            }
        }
    }
}
