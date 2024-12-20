﻿using Microsoft.AspNetCore.Mvc;
using PulseService.Api.Models.Queries;
using PulseService.Api.Models.Responses;
using PulseService.Domain.Adapters;
using PulseService.Domain.Handlers;
using PulseService.Domain.Providers;
using PulseService.Domain.Validation;
using PulseService.Api.Mappers;

namespace PulseService.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserHandler _handler;
    private readonly ITokenManager _tokenManager;
    private readonly ILogger<UserController> _logger;
    private readonly IUserValidationService _validationService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserController(IUserHandler handler, ITokenManager tokenProvider, ILogger<UserController> logger, IUserValidationService validationService, IDateTimeProvider dateTimeProvider)
    {
        _handler = handler;
        _tokenManager = tokenProvider;
        _logger = logger;
        _validationService = validationService;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserQuery newUser, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new user {name}", newUser.Username);

        try
        {
            var validationErrors = await _validationService.GetValidationErrorsAsync(newUser.Username, newUser.Password, cancellationToken);
            if (validationErrors.Any())
            {
                return BadRequest(new ValidationErrorResponse
                {
                    ValidationErrors = validationErrors
                });
            }
                
            var domainUser = newUser.ToDomain();
            domainUser.CreatedAtUtc = _dateTimeProvider.UtcNow;
            
            await _handler.CreateUserAsync(domainUser, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user {name}", newUser.Username);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LogInUser([FromBody] LogInUserQuery logInQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting login for user {name}", logInQuery.Username);

        try
        {
            var user = await _handler.GetUserByCredentialsAsync(logInQuery.ToDomain(), cancellationToken);

            if (user is null)
            {
                return BadRequest("Could not find a user with the given username/password");
            }

            var successResponse = new LoginSuccessfulResponse
            {
                Username = user.Username,
                Token = _tokenManager.GenerateToken(user)
            };

            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed login for user {name}", logInQuery.Username);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}