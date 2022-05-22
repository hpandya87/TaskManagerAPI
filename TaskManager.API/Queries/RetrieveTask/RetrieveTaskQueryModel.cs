using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManager.API.Constants;
using TaskManager.Domain.CoreModels;

namespace TaskManager.API.Queries
{
    [Serializable]
    public class RetrieveTaskQueryModel : IRequest<TaskData>, IValidatableObject
    {
        [Required]
        public string Id { get; set; }

        public RetrieveTaskQueryModel() { }

        public RetrieveTaskQueryModel(string id)
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
