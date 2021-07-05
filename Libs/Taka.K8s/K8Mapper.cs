using System;
using System.Linq;
using AutoMapper;
using k8s.Models;
using Taka.Models;

namespace Taka.K8s
{
    public class K8Mapper : Profile
    {
        public K8Mapper()
        {
            CreateMap<V1Namespace, TakaNamespace>()
                .ForMember(dto => dto.Labels, map => map.MapFrom(source =>
                    source.Metadata.Labels.Select(p => new TakaLabel(p.Key, p.Value)).ToList()
                ))
                .ForMember(dto => dto.Name, map => map.MapFrom(source =>
                    source.Metadata.Name
                ))
                .ForMember(dto => dto.Uid, map => map.MapFrom(source =>
                    source.Metadata.Uid
                ))
                .ForMember(dto => dto.CreationTime, map => map.MapFrom(source =>
                    source.Metadata.CreationTimestamp.Value
                ))
                .ForMember(dto => dto.Status, map => map.MapFrom(source =>
                    source.Status.Phase
                ))
                .ForMember(dto => dto.LatestSyncDateUTC, map => map.MapFrom(src => DateTime.UtcNow))
                .ForMember(dto => dto.DeploymentCount, map => map.MapFrom(src => 0))
                .ForMember(dto => dto.ErrorCount, map => map.MapFrom(src => 0))
                .ForMember(dto => dto.WarningCount, map => map.MapFrom(src => 0))
                .ForMember(dto => dto.ServiceCount, map => map.MapFrom(src => 0))
                //.ForMember(dto => dto.WarningCount, map => map.NullSubstitute(DateTime.UtcNow))
                //.ForMember(dto => dto.LatestSyncDateUTC, map => map.NullSubstitute(DateTime.UtcNow))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}