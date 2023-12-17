using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers;
using FluentValidation;
using System.Text.RegularExpressions;

public class Staff
{
   
    public string? Name { get; set; }

   
    public string? Email { get; set; }

    
    public string? Phone { get; set; }

    
    public decimal? HourlySalary { get; set; }
}
public class StaffValidator : AbstractValidator<Staff>
{
    public StaffValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Personel adý boþ olamaz.")
            .Length(10, 250).WithMessage("Personel adý 10 ile 250 karakter arasýnda olmalýdýr.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarasý boþ olamaz.")
            .Must(BeAValidPhoneNumber).WithMessage("Telefon numarasý geçerli deðil.");

        RuleFor(x => x.HourlySalary)
            .NotNull().WithMessage("Saatlik ücret boþ olamaz.")
            .InclusiveBetween(30, 400).WithMessage("Saatlik ücret izin verilen aralýkta deðil.");
    }

    private bool BeAValidPhoneNumber(string phone)
    {
        // Burada özel telefon numarasý kontrolü yapabilirsiniz
        // Örneðin, regex kullanarak bir kontrol gerçekleþtirebilirsiniz
        // Bu örnek sadece bir kontrol mekanizmasý göstermektedir, gerçek dünyada kullanmadan önce iyileþtirmeler yapmalýsýnýz.
        var regex = new Regex(@"^\d{10}$");
        return regex.IsMatch(phone);
    }
}

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly IValidator<Staff> _validator;

    public StaffController(IValidator<Staff> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public IActionResult Post([FromBody] Staff value)
    {
        var validationResult = _validator.Validate(value);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Staff oluþturma iþlemleri
        // ...

        return Ok("Personel baþarýyla oluþturuldu");
    }
}