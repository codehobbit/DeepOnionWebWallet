using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WWWOnionWallet.Models
{
    public class IndexModel
    {

        #region Properties

        [HiddenInput]
        public string ReturnUrl { get; set; }

        [DataType(DataType.EmailAddress)]
        public string LoginEmail { get; set; }

        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        [DataType(DataType.EmailAddress)]
        public string RegisterEmail { get; set; }

        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        [DataType(DataType.Password)]
        public string RegisterRepeatPassword { get; set; }

        public bool RegisterIsMailing { get; set; }

        public string TwoFactorAuthentication { get; set; }

        [Required]
        public bool RegisterAcceptTOS { get; set; }

        #endregion

        #region Constructor

        public IndexModel()
        {
            this.RegisterIsMailing = true;
        }
        
        #endregion

    }
}