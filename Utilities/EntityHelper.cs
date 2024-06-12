namespace ResuMeAPI.Utilities
{
    public static class EntityHelper
    {
        public static void UpdateEntityProperties<TDto, TEntity>(TDto dto, TEntity entity)
        {
            // Get all the properties for the DTO and the entity
            var dtoProperties = typeof(TDto).GetProperties();
            var entityProperties = typeof(TEntity).GetProperties();

            // Iterate over the DTO properties
            foreach (var dtoProperty in dtoProperties)
            {
                // Find the corresponding property for the entity
                var entityProperty = entityProperties.FirstOrDefault(p => p.Name == dtoProperty.Name);

                if (entityProperty != null && dtoProperty.GetValue(dto) != null)
                {
                    // Update the entity prop value if the entity exists and the DTO prop value is not null
                    entityProperty.SetValue(entity, dtoProperty.GetValue(dto));
                }
            }
        }
    }
}
