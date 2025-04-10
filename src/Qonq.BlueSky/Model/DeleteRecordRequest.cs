﻿using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
	public class DeleteRecordRequest
	{
		[JsonPropertyName("repo")]
		public string Repo { get; set; }
		[JsonPropertyName("collection")]
		public string Collection { get; set; }
		[JsonPropertyName("rkey")]
		public string RecordKey { get; set; }
	}
}
