namespace DsaThreading;

public static class Searches
{
    // Linear search
    // Sorted or unsorted
    // O(n)
    public static int LinearSearch(int[] data, int target)
    {
        for (int i = 0; i < data.Length; i++) if (data[i] == target) return i;
        return -1;
    }
    // Binary search
    // Sorted
    // O(log(n))
    public static int BinarySearch(int[] sortedData, int target)
    {
        int pivot = 0 ;
        return pivot;
    }
}