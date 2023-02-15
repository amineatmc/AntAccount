using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.ValidationRules
{
    public class AllUserValidator:AbstractValidator<AllUser>
    {
      public AllUserValidator()
        {
            RuleFor(c => c.Phone).NotEmpty();
            RuleFor(c => c.Phone).MinimumLength(10);
            RuleFor(c => c.Phone).MaximumLength(11);
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100).MinimumLength(1);
            RuleFor(c => c.Surname).NotEmpty().MaximumLength(100).MinimumLength(1);
            RuleFor(c => c.MailAdress).NotEmpty().MinimumLength(1);
          //  RuleFor(c => c.Password).NotEmpty().MinimumLength(6);
           
            



          
        }
    }
}
