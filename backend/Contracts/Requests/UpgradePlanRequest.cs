using System.ComponentModel.DataAnnotations;
using CloudCore.Common.Models;

namespace CloudCore.Contracts.Requests
{
    public record UpgradePlanRequest
    {
        [Required(ErrorMessage = "New plan is required")]
        [EnumDataType(typeof(SubscriptionPlan), ErrorMessage = "Invalid subscription plan")]
        public SubscriptionPlan NewPlan { get; init; }
    }

}
