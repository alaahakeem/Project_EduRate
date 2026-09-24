using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Queries
{
    public class GetMyFavoritesQuery : IRequest<List<StudentFavoriteDto>>
    {
        public int StudentId { get; set; }
    }

    public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, List<StudentFavoriteDto>>
    {
        private readonly IStudentFavoriteRepository _studentFavoriteRepository;
        public GetMyFavoritesQueryHandler(IStudentFavoriteRepository studentFavoriteRepository) => _studentFavoriteRepository = studentFavoriteRepository;

        public async Task<List<StudentFavoriteDto>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
        {
            return await _studentFavoriteRepository.Query()
                .Include(f => f.Teacher)
                .Include(f => f.Center)
                .Where(f => f.StudentId == request.StudentId)
                .Select(f => new StudentFavoriteDto
                {
                    FavoriteId = f.Id,
                    TeacherId = f.TeacherId,
                    TeacherName = f.Teacher != null ? f.Teacher.Name : null,
                    CenterId = f.CenterId,
                    CenterName = f.Center != null ? f.Center.Name : null
                })
                .ToListAsync(cancellationToken);
        }
    }
}
