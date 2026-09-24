using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Messages.Commands
{
    public class SendMessageCommand : IRequest<string>
    {
        /// <summary>Set by the controller from the caller's JWT.</summary>
        public int SenderId { get; set; }
        /// <summary>Set by the controller from the caller's JWT ("Student" or "Teacher").</summary>
        public string SenderRole { get; set; } = string.Empty;

        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
        }
    }

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, string>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        // Live push over SignalR is triggered from the API layer's MessagesController after
        // the message is persisted here, exactly like the original controller did
        // (it needs IHubContext<ChatHub>, a presentation-layer concern).

        public SendMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                Content = request.Content,
                SentAt = DateTime.Now,
                IsRead = false,
                SenderRole = request.SenderRole
            };

            if (request.SenderRole == "Student")
            {
                message.StudentId = request.SenderId;
                message.TeacherId = request.ReceiverId;
            }
            else if (request.SenderRole == "Teacher")
            {
                message.TeacherId = request.SenderId;
                message.StudentId = request.ReceiverId;
            }

            _messageRepository.Add(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "Message sent successfully.";
        }
    }
}
