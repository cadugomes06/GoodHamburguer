using FluentValidation;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Validators;

public abstract class OrderRequestValidatorBase<T> : AbstractValidator<T>
    where T : IOrderRequest
{
    protected OrderRequestValidatorBase(IMenuRepository menu)
    {
        RuleFor(x => x.SandwichId)
            .Must(id => menu.FindById(id)?.Type == ItemType.Sandwich)
            .WithMessage(x => menu.FindById(x.SandwichId) is null
                ? $"Sanduíche com id {x.SandwichId} não encontrado."
                : $"Item {x.SandwichId} não é um sanduíche.");

        When(x => x.FriesId.HasValue, () =>
            RuleFor(x => x.FriesId!.Value)
                .Must(id => menu.FindById(id)?.Type == ItemType.Fries)
                .WithMessage(x => menu.FindById(x.FriesId!.Value) is null
                    ? $"Acompanhamento com id {x.FriesId} não encontrado."
                    : $"Item {x.FriesId} não é uma batata frita."));

        When(x => x.DrinkId.HasValue, () =>
            RuleFor(x => x.DrinkId!.Value)
                .Must(id => menu.FindById(id)?.Type == ItemType.Drink)
                .WithMessage(x => menu.FindById(x.DrinkId!.Value) is null
                    ? $"Bebida com id {x.DrinkId} não encontrada."
                    : $"Item {x.DrinkId} não é um refrigerante."));

        RuleFor(x => x)
            .Must(x => NoDuplicates(x.SandwichId, x.FriesId, x.DrinkId))
            .WithName("Items")
            .WithMessage("Duplicate item: cada pedido só pode conter um sanduíche, uma batata frita e um refrigerante.");
    }

    private static bool NoDuplicates(int sandwichId, int? friesId, int? drinkId)
    {
        var ids = new List<int> { sandwichId };
        if (friesId.HasValue) ids.Add(friesId.Value);
        if (drinkId.HasValue) ids.Add(drinkId.Value);
        return ids.Distinct().Count() == ids.Count;
    }
}
