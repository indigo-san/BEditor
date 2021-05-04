﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BEditor.Data
{
    /// <summary>
    /// Represents the object that can be stored in Json.
    /// </summary>
    public interface IJsonObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer">Write the data of this object <see cref="Utf8JsonWriter"/>.</param>
        public void GetObjectData(Utf8JsonWriter writer);

        /// <summary>
        /// Set the Json data to this object.
        /// </summary>
        /// <param name="element">Data for this object in Json to be set.</param>
        public void SetObjectData(JsonElement element);
    }
}