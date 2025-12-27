using AutoMapper;
using Sonixy.ChatService.Application.DTOs;
using Sonixy.ChatService.Domain.Entities;

namespace Sonixy.ChatService.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Conversation, ConversationDto>();
        // Note: Participants list in DTO needs manual mapping or robust include logic.
        
        CreateMap<Message, MessageDto>();
        
        CreateMap<ChatParticipant, ParticipantDto>(); 
        // We might need to map User details from external source later.
    }
}
