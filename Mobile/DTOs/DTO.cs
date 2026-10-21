using System.Text.Json.Serialization;

namespace Mobile.DTOs
{
    public class ResponseApi<T>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("msg")]
        public string msg { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class PageData<T>
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("rows")]
        public IEnumerable<T> Rows { get; set; }
    }

    public class ReviewDto
    {
        [JsonPropertyName("reviewId")] public int ReviewId { get; set; }
        [JsonPropertyName("userId")] public int UserId { get; set; }
        [JsonPropertyName("userName")] public string UserName { get; set; } = string.Empty;
        [JsonPropertyName("businessType")] public string BusinessType { get; set; } = string.Empty;
        [JsonPropertyName("businessId")] public int BusinessId { get; set; }
        [JsonPropertyName("rating")] public string? Rating { get; set; }
        [JsonPropertyName("reviewContent")] public string ReviewContent { get; set; } = string.Empty;
        [JsonPropertyName("reviewTime")] public string? ReviewTime { get; set; }
        [JsonPropertyName("imageUrls")] public List<string> ImageUrls { get; set; } = new();
        [JsonPropertyName("likeCount")] public int LikeCount { get; set; }
        [JsonPropertyName("avatarUrl")] public string? AvatarUrl { get; set; }
    }

    public class AddReviewDto
    {
        [JsonPropertyName("businessType")] public string BusinessType { get; set; } = string.Empty;
        [JsonPropertyName("businessId")] public int BusinessId { get; set; }
        [JsonPropertyName("rating")] public string Rating { get; set; } = "5";
        [JsonPropertyName("reviewContent")] public string ReviewContent { get; set; } = string.Empty;
        [JsonPropertyName("imageUrls")] public List<string> ImageUrls { get; set; } = new();
    }

    public class RestaurantDetailDto
    {
        [JsonPropertyName("restaurantId")] public int RestaurantId { get; set; }
        [JsonPropertyName("restaurantName")] public string RestaurantName { get; set; } = string.Empty;
        [JsonPropertyName("priceRange")] public string PriceRange { get; set; } = string.Empty;
        [JsonPropertyName("businessHours")] public string BusinessHours { get; set; } = string.Empty;
        [JsonPropertyName("remainingParkingSpaces")] public int? RemainingParkingSpaces { get; set; }
        [JsonPropertyName("rating")] public string? Rating { get; set; }
        [JsonPropertyName("distance")] public string? Distance { get; set; }
        [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }
    }

    public class ScenicSpotDetailDto
    {
        [JsonPropertyName("spotId")] public int SpotId { get; set; }
        [JsonPropertyName("spotName")] public string SpotName { get; set; } = string.Empty;
        [JsonPropertyName("openingHours")] public string OpeningHours { get; set; } = string.Empty;
        [JsonPropertyName("ticketPrice")] public string TicketPrice { get; set; } = string.Empty;
        [JsonPropertyName("touristPeriod")] public string TouristPeriod { get; set; } = string.Empty;
        [JsonPropertyName("rating")] public string? Rating { get; set; }
        [JsonPropertyName("distance")] public string? Distance { get; set; }
        [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }
    }
}
