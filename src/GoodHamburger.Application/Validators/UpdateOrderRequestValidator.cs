using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;

namespace GoodHamburger.Application.Validators;

public class UpdateOrderRequestValidator : OrderRequestValidatorBase<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(IMenuRepository menu) : base(menu) { }
}
