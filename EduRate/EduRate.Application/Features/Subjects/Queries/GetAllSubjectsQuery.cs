using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Subjects.Queries
{
    public class GetAllSubjectsQuery : IRequest<List<SubjectDto>> { }

    public class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, List<SubjectDto>>
    {
        private readonly ISubjectRepository _subjectRepository;
        public GetAllSubjectsQueryHandler(ISubjectRepository subjectRepository) => _subjectRepository = subjectRepository;

        public async Task<List<SubjectDto>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
        {
            return await _subjectRepository.Query()
                .Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    EducationalStage = s.EducationalStage.ToString()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
