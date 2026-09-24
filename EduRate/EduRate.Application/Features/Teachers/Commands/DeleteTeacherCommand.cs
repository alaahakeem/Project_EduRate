using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;

namespace EduRate.Application.Features.Teachers.Commands
{
    public class DeleteTeacherCommand : IRequest
    {
        public int TeacherId { get; set; }
    }

    public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTeacherCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
        {
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null) throw new NotFoundException("المدرس غير موجود");

            _teacherRepository.Remove(teacher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
