using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    // Returns raw Teacher entities, matching the original SearchTeachers action exactly
    // (its Select-to-DTO mapping was never implemented in the original code).
    public class SearchTeachersQuery : IRequest<List<Teacher>>
    {
        public string? Name { get; set; }
        public string? Subject { get; set; }
    }

    public class SearchTeachersQueryHandler : IRequestHandler<SearchTeachersQuery, List<Teacher>>
    {
        private readonly ITeacherRepository _teacherRepository;
        public SearchTeachersQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

        public async Task<List<Teacher>> Handle(SearchTeachersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Teacher> query = _teacherRepository.Query().Include(t => t.Subject);

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(t => t.Name.Contains(request.Name));

            if (!string.IsNullOrEmpty(request.Subject))
                query = query.Where(t => t.Subject != null && t.Subject.Name.Contains(request.Subject));

            return await query.ToListAsync(cancellationToken);
        }
    }
}
