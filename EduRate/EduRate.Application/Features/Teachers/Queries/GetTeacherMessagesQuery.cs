using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    // Returns raw Message entities, matching the original GetTeacherMessages action exactly.
    public class GetTeacherMessagesQuery : IRequest<List<Message>>
    {
        public int TeacherId { get; set; }
    }

    public class GetTeacherMessagesQueryHandler : IRequestHandler<GetTeacherMessagesQuery, List<Message>>
    {
        private readonly IMessageRepository _messageRepository;
        public GetTeacherMessagesQueryHandler(IMessageRepository messageRepository) => _messageRepository = messageRepository;

        public async Task<List<Message>> Handle(GetTeacherMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _messageRepository.Query()
                .Where(m => m.TeacherId == request.TeacherId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync(cancellationToken);
        }
    }
}
