﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Services
{
    public interface INotificationService
    {
        void StartForegroundServiceCompat();
    }
}
