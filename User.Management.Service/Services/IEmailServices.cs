using System;
using User.Management.Service.Models;

namespace User.Management.Service.Services
{
	public interface IEmailServices
	{
		void SendEmail(Message message);
	}
}

