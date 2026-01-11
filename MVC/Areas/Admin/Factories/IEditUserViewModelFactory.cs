using Business.DTOs;
using MVC.Areas.Admin.Models;

namespace MVC.Areas.Admin.Factories;

public interface IEditUserViewModelFactory
{
    Task<EditUserViewModel> CreateAsync(UserDto user);
    Task PopulateViewModelAsync(EditUserViewModel model, UserDto user);
}

