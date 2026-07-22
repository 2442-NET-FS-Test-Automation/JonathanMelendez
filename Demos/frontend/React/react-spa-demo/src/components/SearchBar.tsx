interface SearchBarProps {
    value: string,
    onChange: (value: string) => void
}

export default function SearchBar({ value, onChange }: SearchBarProps) {
    return (
        <input type="search"
            placeholder="Filter by name"
            value={value}
            onChange={(e) => onChange(e.target.value)}
        />
    );
}