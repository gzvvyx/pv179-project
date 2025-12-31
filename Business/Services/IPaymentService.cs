using Business.DTOs;
using ErrorOr;

namespace Business.Services;

public interface IPaymentService
{
    Task<ErrorOr<PaymentResultDto>> ProcessSubscriptionPaymentAsync(ProcessPaymentDto dto);
    Task<ErrorOr<decimal>> ValidateGiftCardCodeAsync(string code);
}

