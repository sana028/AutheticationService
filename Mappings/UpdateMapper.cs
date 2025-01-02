using AutoMapper;

namespace AuthenticationService.Mappings
{
    public class UpdateMapper<T,P>:Profile
    {
        public UpdateMapper()
        {
            CreateMap<T, P>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => srcMember != null && !Equals(srcMember, destMember)));
        }
    }
}
