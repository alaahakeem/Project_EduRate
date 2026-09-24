using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.PromoCodes.Queries
{
    public class ValidatePromoCodeQuery : IRequest<PromoCodeValidationResultDto>
    {
        public string Code { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public int CenterId { get; set; }
    }

    public class ValidatePromoCodeQueryHandler : IRequestHandler<ValidatePromoCodeQuery, PromoCodeValidationResultDto>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        public ValidatePromoCodeQueryHandler(IPromoCodeRepository promoCodeRepository) => _promoCodeRepository = promoCodeRepository;

        public async Task<PromoCodeValidationResultDto> Handle(ValidatePromoCodeQuery request, CancellationToken cancellationToken)
        {
            var promo = await _promoCodeRepository.Query()
                .FirstOrDefaultAsync(p => p.Code == request.Code && p.IsActive, cancellationToken);

            if (promo == null)
                throw new BadRequestException("كود الخصم غير صحيح أو غير مفعل.");

            if (promo.ExpiryDate < DateTime.Now)
                throw new BadRequestException("انتهت صلاحية كود الخصم.");

            if (promo.CurrentUsageCount >= promo.MaxUsageCount)
                throw new BadRequestException("تم الوصول للحد الأقصى لاستخدام هذا الكود.");

            // Preserved as-is: originally commented out in the source (TeacherId/CenterId
            // scoping was not actually enforced).
            // if (promo.TeacherId != null && promo.TeacherId != request.TeacherId)
            //     throw new BadRequestException("كود الخصم هذا غير صالح لهذا المدرس.");
            // if (promo.CenterId != null && promo.CenterId != request.CenterId)
            //     throw new BadRequestException("كود الخصم هذا غير صالح لهذا السنتر.");

            return new PromoCodeValidationResultDto
            {
                Message = "كود الخصم صالح!",
                DiscountPercentage = promo.DiscountPercentage
            };
        }
    }
}
