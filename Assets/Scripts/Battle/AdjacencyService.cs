using System.Collections.Generic;

/// <summary>
/// The AdjacencyService class is responsible for assigning adjacent allies to entities in a team based on predefined adjacency logic.
/// This adjacency logic determines which entities are considered neighbors based on their position in the team.
/// </summary>
public class AdjacencyService
{
    /// <summary>
    /// Assigns adjacent allies to each entity in the team based on fixed adjacency rules:
    /// - Entity 1 is adjacent to Entity 2 and Entity 3.
    /// - Entity 2 is adjacent to Entity 1 and Entity 4.
    /// - Entity 3 is adjacent to Entity 1 and Entity 5.
    /// - Entity 4 is adjacent to Entity 2.
    /// - Entity 5 is adjacent to Entity 3.
    /// </summary>
    /// <param name="pTeam">The list of entities representing the team.</param>
    public void AssignAdjacency(List<Entity> pTeam)
    {
        // Return early if the team is null or empty.
        if (pTeam == null || pTeam.Count == 0)
            return;

        // Iterate through each entity in the team and assign adjacent allies.
        for (int i = 0; i < pTeam.Count; i++)
        {
            // Clear the current list of adjacent allies for the entity.
            pTeam[i].AdjacentAllies.Clear();

            // Assign adjacent allies based on the entity's position in the team.
            switch (i)
            {
                case 0: // Entity 1
                    AddIfAlive(pTeam[i], pTeam, 1); // Add Entity 2 if alive.
                    AddIfAlive(pTeam[i], pTeam, 2); // Add Entity 3 if alive.
                    break;
                case 1: // Entity 2
                    AddIfAlive(pTeam[i], pTeam, 0); // Add Entity 1 if alive.
                    AddIfAlive(pTeam[i], pTeam, 3); // Add Entity 4 if alive.
                    break;
                case 2: // Entity 3
                    AddIfAlive(pTeam[i], pTeam, 0); // Add Entity 1 if alive.
                    AddIfAlive(pTeam[i], pTeam, 4); // Add Entity 5 if alive.
                    break;
                case 3: // Entity 4
                    AddIfAlive(pTeam[i], pTeam, 1); // Add Entity 2 if alive.
                    break;
                case 4: // Entity 5
                    AddIfAlive(pTeam[i], pTeam, 2); // Add Entity 3 if alive.
                    break;
            }
        }
    }

    /// <summary>
    /// Adds an entity to the source entity's adjacent allies list if the entity at the specified index is alive.
    /// </summary>
    /// <param name="pSource">The source entity to which adjacent allies are being added.</param>
    /// <param name="pTeam">The list of entities representing the team.</param>
    /// <param name="pIndex">The index of the entity to check and add.</param>
    private void AddIfAlive(Entity pSource, List<Entity> pTeam, int pIndex)
    {
        // Ensure the index is within bounds and the entity is not dead.
        if (pIndex >= 0 && pIndex < pTeam.Count && !pTeam[pIndex].IsDead)
        {
            // Add the entity to the source's adjacent allies list.
            pSource.AdjacentAllies.Add(pTeam[pIndex]);
        }
    }
}
