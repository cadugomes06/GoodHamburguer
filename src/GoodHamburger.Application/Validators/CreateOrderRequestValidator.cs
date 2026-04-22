using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;

namespace GoodHamburger.Application.Validators;

public class CreateOrderRequestValidator : OrderRequestValidatorBase<CreateOrderRequest>
{
    public CreateOrderRequestValidator(IMenuRepository menu) : base(menu) { }
}
