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
            .NotEmpty().WithMessage("Ýsim boþ olamaz.")
            .Length(10, 250).WithMessage("Ýsim 10 ile 250 karakter arasýnda olmalýdýr.")
            .WithName("Name"); // FluentValidation için isim tanýmlamasý

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Doðum tarihi boþ olamaz.")
            .Must(BeValidBirthDate).WithMessage("Doðum tarihi geçerli deðil.")
            .WithName("DateOfBirth"); // FluentValidation için isim tanýmlamasý

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
            .WithName("Email"); // FluentValidation için isim tanýmlamasý

        RuleFor(x => x.Phone)
           .NotEmpty().WithMessage("Telefon numarasý boþ olamaz.")
           .Must(BeAValidPhoneNumber).WithMessage("Telefon numarasý geçerli deðil.");

        RuleFor(x => x.HourlySalary)
    .InclusiveBetween(50, 400).WithMessage("Saatlik ücret izin verilen aralýkta deðil.")
    .Must((employee, hourlySalary) => BeAValidHourlySalary(hourlySalary, employee))
    .WithMessage("Minimum saatlik ücret kriterleri saðlanmýyor.")
    .WithName("HourlySalary");
    }
    private bool BeAValidPhoneNumber(string phone)
    {
        // Burada özel telefon numarasý kontrolü yapabilirsiniz
        // Örneðin, regex kullanarak bir kontrol gerçekleþtirebilirsiniz
        // Bu örnek sadece bir kontrol mekanizmasý göstermektedir, gerçek dünyada kullanmadan önce iyileþtirmeler yapmalýsýnýz.
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

        // Employee oluþturma iþlemleri
        // ...

        return Ok("Çalýþan baþarýyla oluþturuldu");
    }
}