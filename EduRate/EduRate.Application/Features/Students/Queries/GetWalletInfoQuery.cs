using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;

namespace EduRate.Application.Features.Students.Queries
{
    public class GetWalletInfoQuery : IRequest<WalletInfoDto>
    {
        public int StudentId { get; set; }
    }

    public class GetWalletInfoQueryHandler : IRequestHandler<GetWalletInfoQuery, WalletInfoDto>
    {
        private readonly IStudentRepository _studentRepository;
        public GetWalletInfoQueryHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

        public async Task<WalletInfoDto> Handle(GetWalletInfoQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("Student not found.");

            return new WalletInfoDto
            {
                WalletBalance = student.WalletBalance,
                RewardPoints = student.RewardPoints
            };
        }
    }
}
