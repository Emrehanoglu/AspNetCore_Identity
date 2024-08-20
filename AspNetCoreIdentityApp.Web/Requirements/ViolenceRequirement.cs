using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolenceRequirement:IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }
    }

    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "birthdate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var birthdateClaim = context.User.Claims.FirstOrDefault(x => x.Type == "birthdate");

            //yaş hesaplaması yapıldı
            var today = DateTime.Now;
            var birthDate = Convert.ToDateTime(birthdateClaim!.Value);
            var age = today.Year - birthDate.Year;
            //asagıdada artık yıl hesabı yapıldı
            //subat ayının 4 yılda bir 29 cekmesinden kaynaklı
            if(birthDate > today.AddYears(-age))
            {
                age--;
            }

            if (requirement.ThresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
