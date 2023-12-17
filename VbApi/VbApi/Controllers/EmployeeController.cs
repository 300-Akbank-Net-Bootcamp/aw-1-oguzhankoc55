using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers;

using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class Employee 
{
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public double HourlySalary { get; set; }

}


public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("�sim bo� olamaz.")
            .Length(10, 250).WithMessage("�sim 10 ile 250 karakter aras�nda olmal�d�r.")
            .WithName("Name"); // FluentValidation i�in isim tan�mlamas�

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Do�um tarihi bo� olamaz.")
            .Must(BeValidBirthDate).WithMessage("Do�um tarihi ge�erli de�il.")
            .WithName("DateOfBirth"); // FluentValidation i�in isim tan�mlamas�

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email bo� olamaz.")
            .EmailAddress().WithMessage("Ge�erli bir email adresi giriniz.")
            .WithName("Email"); // FluentValidation i�in isim tan�mlamas�

        RuleFor(x => x.Phone)
           .NotEmpty().WithMessage("Telefon numaras� bo� olamaz.")
           .Must(BeAValidPhoneNumber).WithMessage("Telefon numaras� ge�erli de�il.");

        RuleFor(x => x.HourlySalary)
    .InclusiveBetween(50, 400).WithMessage("Saatlik �cret izin verilen aral�kta de�il.")
    .Must((employee, hourlySalary) => BeAValidHourlySalary(hourlySalary, employee))
    .WithMessage("Minimum saatlik �cret kriterleri sa�lanm�yor.")
    .WithName("HourlySalary");
    }
    private bool BeAValidPhoneNumber(string phone)
    {
        // Burada �zel telefon numaras� kontrol� yapabilirsiniz
        // �rne�in, regex kullanarak bir kontrol ger�ekle�tirebilirsiniz
        // Bu �rnek sadece bir kontrol mekanizmas� g�stermektedir, ger�ek d�nyada kullanmadan �nce iyile�tirmeler yapmal�s�n�z.
        var regex = new Regex(@"^\d{10}$");
        return regex.IsMatch(phone);
    }
    private bool BeValidBirthDate(DateTime dateOfBirth)
    {
        var minAllowedBirthDate = DateTime.Today.AddYears(-65);
        return minAllowedBirthDate <= dateOfBirth;
    }

    private bool BeAValidHourlySalary(double hourlySalary, Employee employee)
    {
        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirdyYears = employee.DateOfBirth <= dateBeforeThirtyYears;

        return isOlderThanThirdyYears ? hourlySalary >= 200 : hourlySalary >= 50;
    }
}
public class MinLegalSalaryRequiredAttribute : ValidationAttribute
{
    public MinLegalSalaryRequiredAttribute(double minJuniorSalary, double minSeniorSalary)
    {
        MinJuniorSalary = minJuniorSalary;
        MinSeniorSalary = minSeniorSalary;
    }

    public double MinJuniorSalary { get; }
    public double MinSeniorSalary { get; }
    public string GetErrorMessage() => $"Minimum hourly salary is not valid.";

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        var employee = (Employee)validationContext.ObjectInstance;
        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirdyYears = employee.DateOfBirth <= dateBeforeThirtyYears;
        var hourlySalary = (double)value;

        var isValidSalary = isOlderThanThirdyYears ? hourlySalary >= MinSeniorSalary : hourlySalary >= MinJuniorSalary;

        return isValidSalary ? ValidationResult.Success : new ValidationResult(GetErrorMessage());
    }
}
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IValidator<Employee> _validator;

    public EmployeeController(IValidator<Employee> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public IActionResult Post([FromBody] Employee value)
    {
        var validationResult = _validator.Validate(value);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Employee olu�turma i�lemleri
        // ...

        return Ok("�al��an ba�ar�yla olu�turuldu");
    }
}