using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Subjects.Commands
{
    public class DeleteSubjectCommand : IRequest<string>
    {
        public int SubjectId { get; set; }
    }

    public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, string>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubjectCommandHandler(ISubjectRepository subjectRepository, ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null) throw new NotFoundException("المادة غير موجودة.");

            var hasTeachers = await _teacherRepository.Query().AnyAsync(t => t.SubjectId == request.SubjectId, cancellationToken);
            if (hasTeachers)
                throw new BadRequestException("لا يمكن حذف هذه المادة لوجود مدرسين مرتبطين بها.");

            _subjectRepository.Remove(subject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "تم حذف المادة بنجاح.";
        }
    }
}
