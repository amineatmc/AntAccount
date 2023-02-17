using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.ValidationRules
{
    public class AllUserValidator : AbstractValidator<AllUser>
    {
        public AllUserValidator()
        {
            RuleFor(c => c.Phone).NotEmpty().WithErrorCode("406").WithMessage("Boş Geçilemez.");
            RuleFor(c => c.Phone).MinimumLength(10).WithErrorCode("406").WithMessage("Geçersiz uzunluk. ");
            RuleFor(c => c.Phone).MaximumLength(11).WithErrorCode("406").WithMessage("Geçersiz uzunluk.");
            RuleFor(c => c.Name).NotEmpty().WithErrorCode("406").WithMessage("Boş Geçilemez.");
            RuleFor(c => c.Name).MaximumLength(100).MinimumLength(1).WithErrorCode("406").WithMessage("Geçersiz uzunluk.");
            RuleFor(c => c.Surname).NotEmpty().WithErrorCode("406").WithMessage("Boş Geçilemez.");
            RuleFor(c => c.Surname).MaximumLength(100).MinimumLength(1).WithErrorCode("406").WithMessage("Geçersiz uzunluk.");
            RuleFor(c => c.MailAdress).NotEmpty().WithErrorCode("406").WithMessage("Boş Geçilemez.");
            RuleFor(c=>c.MailAdress).EmailAddress();
            RuleFor(c => c.Password).NotEmpty().WithErrorCode("406").WithMessage("Boş Geçilemez.");
            RuleFor(c => c.Password).MinimumLength(6).WithErrorCode("406").WithMessage("Geçersiz uzunluk.");
            RuleFor(c => c.Password).MaximumLength(15).WithErrorCode("406").WithMessage("Geçersiz uzunluk.");

        }
    }
}
