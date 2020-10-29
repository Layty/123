using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace ToDoWebApi.Core
{
    public class DlmsData
    {
        [Key] public int Id { get; set; }

        public string DataName { get; set; }
        public string ClassId { get; set; }
        public string LogicName { get; set; }
        public byte Attr { get; set; }
    }
}