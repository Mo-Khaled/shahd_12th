export default function FormField({ label, ...props }) {
  return (
    <label className="form-field">
      <span>{label}</span>
      <input {...props} />
    </label>
  )
}
