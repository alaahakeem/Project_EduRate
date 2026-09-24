using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Messages.Queries
{
    public class GetConversationQuery : IRequest<List<MessageDto>>
    {
        /// <summary>Set by the controller from the caller's JWT.</summary>
        public int MyId { get; set; }
        public string MyRole { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
    }

    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, List<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        public GetConversationQueryHandler(IMessageRepository messageRepository) => _messageRepository = messageRepository;

        public async Task<List<MessageDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            int searchStudentId = request.MyRole == "Student" ? request.MyId : request.ReceiverId;
            int searchTeacherId = request.MyRole == "Teacher" ? request.MyId : request.ReceiverId;

            return await _messageRepository.Query()
                .Where(m => m.StudentId == searchStudentId && m.TeacherId == searchTeacherId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderRole = m.SenderRole,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .ToListAsync(cancellationToken);
        }
    }
}
