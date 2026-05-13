const fs = require('fs');
const buf = fs.readFileSync('docs.json');
// Check if it's UTF-16LE or UTF-8
const isUTF16LE = buf[0] === 0xFF && buf[1] === 0xFE || buf[1] === 0;
let text = isUTF16LE ? buf.toString('utf16le') : buf.toString('utf8');
text = text.replace(/^\uFEFF/, ''); // remove BOM
const jsonStart = text.indexOf('['); // Since it returns an array of results
const jsonEnd = text.lastIndexOf(']') + 1;
const docs = JSON.parse(text.substring(jsonStart, jsonEnd));

docs.forEach(doc => {
  console.log('--- ' + doc.component + ' ---');
  // Just print the first usage block or anatomy block
  const lines = doc.content.split('\n');
  let inUsage = false;
  let codeBlock = '';
  for (let line of lines) {
    if (line.includes('## Usage') || line.includes('## Anatomy')) inUsage = true;
    if (inUsage && line.startsWith('```tsx')) {
       codeBlock += line + '\n';
    } else if (inUsage && line.startsWith('```') && codeBlock) {
       codeBlock += line + '\n';
       break;
    } else if (codeBlock) {
       codeBlock += line + '\n';
    }
  }
  console.log(codeBlock);
});
