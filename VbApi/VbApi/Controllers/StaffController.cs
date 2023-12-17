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
            .NotEmpty().WithMessage("Personel ad� bo� olamaz.")
            .Length(10, 250).WithMessage("Personel ad� 10 ile 250 karakter aras�nda olmal�d�r.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi bo� olamaz.")
            .EmailAddress().WithMessage("Ge�erli bir email adresi giriniz.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numaras� bo� olamaz.")
            .Must(BeAValidPhoneNumber).WithMessage("Telefon numaras� ge�erli de�il.");

        RuleFor(x => x.HourlySalary)
            .NotNull().WithMessage("Saatlik �cret bo� olamaz.")
            .InclusiveBetween(30, 400).WithMessage("Saatlik �cret izin verilen aral�kta de�il.");
    }

    private bool BeAValidPhoneNumber(string phone)
    {
        // Burada �zel telefon numaras� kontrol� yapabilirsiniz
        // �rne�in, regex kullanarak bir kontrol ger�ekle�tirebilirsiniz
        // Bu �rnek sadece bir kontrol mekanizmas� g�stermektedir, ger�ek d�nyada kullanmadan �nce iyile�tirmeler yapmal�s�n�z.
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

        // Staff olu�turma i�lemleri
        // ...

        return Ok("Personel ba�ar�yla olu�turuldu");
    }
}